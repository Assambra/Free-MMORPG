package com.assambra.masterserver.app.model.request;

import lombok.Builder;
import lombok.Getter;

@Getter
@Builder
public class RequestForgotPasswordModel {
    private String usernameOrEMail;
}
