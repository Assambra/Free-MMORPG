package com.assambra.game.app.model;

import lombok.Builder;
import lombok.Getter;

import java.util.List;

@Getter
@Builder
public class PlayModel {
    private List<String> userNames;
    private List<CharacterSpawnModel> characterSpawns;
}
