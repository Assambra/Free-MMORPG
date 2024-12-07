package com.assambra.masterserver.app.converter;

import com.assambra.masterserver.app.model.request.RequestAccountActivationModel;
import com.assambra.masterserver.app.model.request.RequestForgotPasswordModel;
import com.assambra.masterserver.app.request.ActivateAccountRequest;
import com.assambra.masterserver.app.request.ForgotPasswordRequest;
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

    public RequestForgotPasswordModel toModel(ForgotPasswordRequest request)
    {
        return RequestForgotPasswordModel.builder()
                .usernameOrEMail(request.getUsernameOrEMail())
                .build();
    }
}
