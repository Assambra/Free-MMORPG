package com.assambra.masterserver.app.model;

import lombok.Builder;
import lombok.Getter;

import java.util.List;

@Getter
@Builder
public class CharacterInfoListModel {
    private List<CharacterInfoModel> characters;
}
