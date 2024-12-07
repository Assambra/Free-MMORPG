package com.assambra.masterserver.app.converter;

import com.assambra.masterserver.app.model.request.RequestAccountActivationModel;
import com.assambra.masterserver.app.request.ActivateAccountRequest;
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
}
