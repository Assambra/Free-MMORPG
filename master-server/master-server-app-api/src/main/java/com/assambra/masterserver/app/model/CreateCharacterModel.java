package com.assambra.masterserver.app.model;

import lombok.Builder;
import lombok.Getter;

@Getter
@Builder
public class CreateCharacterModel {
    private Long id;
    private String result;
}
