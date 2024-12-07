package com.assambra.masterserver.app.converter;

import com.assambra.masterserver.app.model.request.*;
import com.assambra.masterserver.app.request.*;
import com.tvd12.ezyfox.bean.annotation.EzySingleton;
import lombok.AllArgsConstructor;

@EzySingleton
@AllArgsConstructor
public class RequestToModelConverter {

    public RequestCreateAccountModel toModel(CreateAccountRequest request) {
        return RequestCreateAccountModel.builder()
                .email(request.getEmail())
                .username(request.getUsername())
                .password(request.getPassword())
                .build();
    }

    public RequestAccountActivationModel toModel(ActivateAccountRequest request) {
        return RequestAccountActivationModel.builder()
                .activationCode(request.getActivationCode())
                .build();
    }

    public RequestForgotPasswordModel toModel(ForgotPasswordRequest request) {
        return RequestForgotPasswordModel.builder()
                .usernameOrEMail(request.getUsernameOrEMail())
                .build();
    }

    public RequestForgotUsernameModel toModel(ForgotUsernameRequest request) {
        return RequestForgotUsernameModel.builder()
                .email(request.getEmail())
                .build();
    }

    public RequestCreateCharacterModel toModel(CreateCharacterRequest request) {
        return RequestCreateCharacterModel.builder()
                .name(request.getName())
                .sex(request.getSex())
                .race(request.getRace())
                .model(request.getModel())
                .build();
    }

    public RequestPlayModel toModel(PlayRequest request) {
        return RequestPlayModel.builder()
                .playerId(request.getPlayerId())
                .build();
    }

    public RequestChangeServerModel toModel(ChangeServerRequest request) {
        return RequestChangeServerModel.builder()
                .playerId(request.getPlayerId())
                .room(request.getRoom())
                .position(request.getPosition())
                .rotation(request.getRotation())
                .build();
    }

    public RequestServerReadyModel toModel(ServerReadyRequest request)
    {
        return RequestServerReadyModel.builder()
                .password(request.getPassword())
                .build();
    }
}
