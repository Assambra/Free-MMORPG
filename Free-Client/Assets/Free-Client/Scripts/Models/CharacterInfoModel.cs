namespace Assambra.FreeClient
{
    public class CharacterInfoModel
    {
        public long Id { get => _id; }
        public string Name { get => _name; }
        public string Sex { get => _sex; }
        public string Race { get => _race; }
        public string Model { get => _model; }
        public string Room { get => _room; }
        
        private long _id;
        private string _name;
        private string _sex;
        private string _race;
        private string _model;
        private string _room;

        public CharacterInfoModel(long id, string name, string sex, string race, string model, string room)
        {
            this._id = id;
            this._name = name;
            this._sex = sex;
            this._race = race;
            this._model = model;
            this._room = room;
        }
    }
}

