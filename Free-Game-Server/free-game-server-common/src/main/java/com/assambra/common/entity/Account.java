package com.assambra.common.entity;

import com.tvd12.ezyfox.annotation.EzyId;
import com.tvd12.ezyfox.database.annotation.EzyCollection;
import lombok.Data;

@Data
@EzyCollection
public class Account {
    @EzyId
    Long id;

    String email;
    String username;
    String password;
}
