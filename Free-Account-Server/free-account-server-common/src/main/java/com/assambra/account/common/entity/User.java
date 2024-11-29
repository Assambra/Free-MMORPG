package com.assambra.account.common.entity;

import com.tvd12.ezyfox.annotation.EzyId;
import com.tvd12.ezyfox.database.annotation.EzyCollection;
import lombok.Data;

import java.util.Date;

@Data
@EzyCollection
public class User {
    @EzyId
    Long id;

    String email;
    String username;
    String password;
    Boolean activated;
    String activationCode;
    Date creationDate;
    String role;
    Integer maxAllowedCharacters;
}