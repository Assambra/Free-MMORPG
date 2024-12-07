package com.assambra.masterserver.app.model.request;

import lombok.Builder;
import lombok.Getter;

@Getter
@Builder
public class RequestCreateCharacterModel {
    private String name;
    private String sex;
    private String race;
    private String model;
}
