package com.assambra.masterserver.app.model.request;

import lombok.Builder;
import lombok.Getter;

@Getter
@Builder
public class RequestCreateAccountModel {
    private String email;
    private String username;
    private String password;
}
