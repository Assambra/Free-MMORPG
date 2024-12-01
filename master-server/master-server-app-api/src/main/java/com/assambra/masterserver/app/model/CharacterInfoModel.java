package com.assambra.masterserver.app.model;

import lombok.Builder;
import lombok.Getter;

@Getter
@Builder
public class CharacterInfoModel {
    private Long id;
    private String name;
    private String sex;
    private String race;
    private String model;
    private String room;
}
