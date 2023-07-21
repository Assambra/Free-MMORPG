﻿using System;
using System.Collections;
using System.Collections.Generic;
using com.tvd12.ezyfoxserver.client.entity;
using com.tvd12.ezyfoxserver.client.factory;

namespace com.tvd12.ezyfoxserver.client.binding
{
    /// <summary>
    /// Convert EzyArray or EzyObject to C# Object and
    /// Convert C# Object to EzyArray or EzyObject
    /// </summary>
    /// <example>
    /// <c>Example:</c>
    /// <code>
    /// public class BindingExample
    /// {
    ///     public void Run()
    ///     {
    ///         var binding = EzyBinding.builder()
    ///             .addReflectionMapConverter<User>()
    ///             .addReflectionArrayConverter<Room>()
    ///             .build();
    ///         var user = new User
    ///         {
    ///             Name = "Monkey",
    ///             Age = 29,
    ///             Friend = "Fox"
    ///         };
    /// 
    ///         var map = binding.marshall<EzyObject>(user);
    ///         Console.WriteLine("user to map: " + map);
    /// 
    ///         var mappedUser = binding.unmarshall<User>(map);
    ///         Console.WriteLine("map to user: " + mappedUser);
    /// 
    ///         var room = new Room()
    ///         {
    ///             Id = 1,
    ///             Name = "Lobby"
    ///         };
    /// 
    ///         var array = binding.marshall<EzyArray>(room);
    ///         Console.WriteLine("room to array: " + array);
    /// 
    ///         var mappedRoom = binding.unmarshall<Room>(array);
    ///         Console.WriteLine("map to room: " + mappedRoom);
    ///     }
    /// }
    /// 
    /// public class User
    /// {
    ///     public string Name { get; set; }
    ///     public int Age { get; set; }
    ///     [EzyValue("f")]
    ///     public String Friend { get; set; }
    /// 
    ///     public override string ToString()
    ///     {
    ///         return "User (\n" +
    ///             "\tName: " + Name +
    ///             "\n\tAge: " + Age +
    ///             "\n\tFriend: " + Friend +
    ///             "\n)";
    ///     }
    /// }
    /// 
    /// public class Room
    /// {
    ///     [EzyValue(0)]
    ///     public long Id { get; set; }
    /// 
    ///     [EzyValue(1)]
    ///     public String Name { get; set; }
    /// 
    ///     public override string ToString()
    ///     {
    ///         return "Room (\n" +
    ///             "\tId: " + Id +
    ///             "\n\tName: " + Name +
    ///             "\n)";
    ///     }
    /// }/// 
    /// </code>
    /// </example>
    public class EzyBinding
    {
        private readonly EzyMarshaller marshaller;
        private readonly EzyUnmarshaller unmarshaller;

        public EzyBinding(
            IDictionary<Type, IEzyWriter> writerByInType,
            IDictionary<Type, IEzyReader> readerByOutType)
        {
            this.marshaller = new EzyMarshaller(writerByInType);
            this.unmarshaller = new EzyUnmarshaller(readerByOutType);
        }

        public static EzyBindingBuilder builder()
        {
            return new EzyBindingBuilder();
        }

        public T marshall<T>(object input)
        {
            return marshaller.marshall<T>(input);
        }

        public List<object> marshallToList(object input)
        {
            return marshaller.marshall<EzyArray>(input).toList<object>();
        }

        public Dictionary<object, object> marshallToDict(object input)
        {
            return marshaller.marshall<EzyObject>(input).toDict<object, object>();
        }

        public T unmarshall<T>(object input)
        {
            return unmarshaller.unmarshall<T>(input);
        }

        public object unmarshall(object input, Type outType)
        {
            return unmarshaller.unmarshallByOutType(input, outType);
        }
    }

    public class EzyBindingBuilder
    {
        private readonly IDictionary<Type, IEzyWriter> writerByInType;
        private readonly IDictionary<Type, IEzyReader> readerByOutType;

        public EzyBindingBuilder()
        {
            this.writerByInType = new Dictionary<Type, IEzyWriter>();
            this.readerByOutType = new Dictionary<Type, IEzyReader>();
            this.addConverter(new EzyDateTimeConverter());
        }

        public EzyBindingBuilder addReader(IEzyReader reader)
        {
            this.readerByOutType[reader.getOutType()] = reader;
            return this;
        }

        public EzyBindingBuilder addWriter(IEzyWriter writer)
        {
            this.writerByInType[writer.getInType()] = writer;
            return this;
        }

        public EzyBindingBuilder addConverter(IEzyConverter converter)
        {
            this.readerByOutType[converter.getOutType()] = converter;
            this.writerByInType[converter.getInType()] = converter;
            return this;
        }

        public EzyBindingBuilder addReflectionMapConverter<T>()
        {
            return addConverter(new EzyReflectionMapConverter<T>());
        }

        public EzyBindingBuilder addReflectionArrayConverter<T>()
        {
            return addConverter(new EzyReflectionArrayConverter<T>());
        }

        public EzyBindingBuilder addReflectionConverter<T>()
        {
            Type type = typeof(T);
            object[] attributes = type.GetCustomAttributes(false);
            foreach (object attr in attributes)
            {
                if (attr.GetType() == typeof(EzyObjectBinding))
                {
                    return addConverter(new EzyReflectionMapConverter<T>());
                }
                if (attr.GetType() == typeof(EzyArrayBinding))
                {
                    return addConverter(new EzyReflectionArrayConverter<T>());
                }

            }
            return this;
        }

        public EzyBinding build()
        {
            return new EzyBinding(writerByInType, readerByOutType);
        }
    }

    public class EzyMarshaller
    {
        private readonly IDictionary<Type, IEzyWriter> writerByInType;

        public EzyMarshaller(IDictionary<Type, IEzyWriter> writerByInType)
        {
            this.writerByInType = writerByInType;
        }

        public T marshall<T>(object input)
        {
            return (T)marshallByInType(input, input.GetType());
        }

        public object marshallByInType(object input, Type inType)
        {
            if (input == null)
            {
                return null;
            }
            if (writerByInType.ContainsKey(inType))
            {
                IEzyWriter writer = writerByInType[inType];
                return writer.write(input, this);
            }
            if (typeof(IDictionary).IsAssignableFrom(inType)) {
                EzyObject answer = EzyEntityFactory.newObject();
                IDictionary dict = (IDictionary)(input);
                Type keyType = inType.GetGenericArguments()[0];
                Type valueType = inType.GetGenericArguments()[1];
                foreach (DictionaryEntry entry in dict)
                {
                    answer.put(
                        marshallByInType(entry.Key, keyType),
                        marshallByInType(entry.Value, valueType));
                }
                return answer;
            }
            else if (typeof(IList).IsAssignableFrom(inType))
            {
                EzyArray answer = EzyEntityFactory.newArray();
                IList list = (IList)(input);
                Type valueType = inType.GetGenericArguments()[0];
                foreach (Object value in list)
                {
                    answer.add(marshallByInType(value, valueType));
                }
                return answer;
            }
            return input;
        }
    }

    public class EzyUnmarshaller
    {
        private readonly IDictionary<Type, IEzyReader> readerByOutType;

        public EzyUnmarshaller(IDictionary<Type, IEzyReader> readerByOutType)
        {
            this.readerByOutType = readerByOutType;
        }

        public T unmarshall<T>(object input)
        {
            return (T)unmarshallByOutType(input, typeof(T));
        }

        public object unmarshallByOutType(object input, Type outType)
        {
            if (input == null)
            {
                return null;
            }
            if (readerByOutType.ContainsKey(outType))
            {
                IEzyReader reader = readerByOutType[outType];
                return reader.read(input, this);
            }
            if (outType.IsGenericType)
            {
                if (typeof(IDictionary).IsAssignableFrom(outType) ||
                    typeof(IDictionary<,>) == outType.GetGenericTypeDefinition())
                {
                    Type dictType = typeof(Dictionary<,>);
                    Type constructed = dictType.MakeGenericType(outType.GetGenericArguments());
                    IDictionary answer = (IDictionary)Activator.CreateInstance(constructed);
                    EzyObject obj = (EzyObject)input;
                    Type keyType = outType.GetGenericArguments()[0];
                    Type valueType = outType.GetGenericArguments()[1];
                    foreach (object key in obj.keys())
                    {
                        answer[unmarshallByOutType(key, keyType)] =
                            unmarshallByOutType(obj.getByOutType(key, valueType), valueType);
                    }
                    return answer;
                }
                else if (typeof(IList).IsAssignableFrom(outType) ||
                    typeof(IList<>) == outType.GetGenericTypeDefinition())
                {
                    Type listType = typeof(List<>);
                    Type constructed = listType.MakeGenericType(outType.GetGenericArguments());
                    IList answer = (IList)Activator.CreateInstance(constructed);
                    EzyArray array = (EzyArray)input;
                    Type valueType = outType.GetGenericArguments()[0];
                    for (int i = 0; i < array.size(); ++i)
                    {
                        Object rawValue = array.getByOutType(i, valueType);
                        Object value = unmarshallByOutType(rawValue, valueType);
                        answer.Add(value);
                    }
                    return answer;
                }
            }
            return input;
        }

        public IList<T> unmarshallList<T>(EzyArray array)
        {
            List<T> answer = new List<T>();
            for (int i = 0; i < array.size(); ++i)
            {
                answer.Add(unmarshall<T>(array.get<object>(i)));
            }
            return answer;
        }

        public IDictionary<K, V> unmarshallDict<K, V>(EzyObject obj)
        {
            IDictionary<K, V> answer = new Dictionary<K, V>();
            foreach (object key in obj.keys())
            {
                answer[unmarshall<K>(key)] = unmarshall<V>(obj.get<V>(key));
            }
            return answer;
        }
    }
}
