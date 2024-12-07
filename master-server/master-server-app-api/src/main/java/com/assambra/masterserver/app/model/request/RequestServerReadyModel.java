package com.assambra.masterserver.app.model.request;

import lombok.Builder;
import lombok.Getter;

@Getter
@Builder
public class RequestServerReadyModel {
    private String password;
}
