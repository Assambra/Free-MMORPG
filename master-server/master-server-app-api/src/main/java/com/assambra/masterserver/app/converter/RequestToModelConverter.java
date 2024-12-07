package com.assambra.masterserver.app.converter;

import com.assambra.masterserver.app.model.request.RequestAccountActivationModel;
import com.assambra.masterserver.app.model.request.RequestCreateAccountModel;
import com.assambra.masterserver.app.model.request.RequestForgotPasswordModel;
import com.assambra.masterserver.app.model.request.RequestForgotUsernameModel;
import com.assambra.masterserver.app.request.ActivateAccountRequest;
import com.assambra.masterserver.app.request.CreateAccountRequest;
import com.assambra.masterserver.app.request.ForgotPasswordRequest;
import com.assambra.masterserver.app.request.ForgotUsernameRequest;
import com.tvd12.ezyfox.bean.annotation.EzySingleton;
import lombok.AllArgsConstructor;

@EzySingleton
@AllArgsConstructor
public class RequestToModelConverter {

    public RequestAccountActivationModel toModel(ActivateAccountRequest request)
    {
        return RequestAccountActivationModel.builder()
                .activationCode(request.getActivationCode())
                .build();
    }

    public RequestCreateAccountModel toModel(CreateAccountRequest request)
    {
        return RequestCreateAccountModel.builder()
                .email(request.getEmail())
                .username(request.getUsername())
                .password(request.getPassword())
                .build();

    }

    public RequestForgotPasswordModel toModel(ForgotPasswordRequest request)
    {
        return RequestForgotPasswordModel.builder()
                .usernameOrEMail(request.getUsernameOrEMail())
                .build();
    }

    public RequestForgotUsernameModel toModel(ForgotUsernameRequest request)
    {
        return RequestForgotUsernameModel.builder()
                .email(request.getEmail())
                .build();
    }
}
