package com.assambra.game.app.model;

import lombok.Builder;
import lombok.Getter;

@Getter
@Builder
public class PlayerInputModel {
    private int time;
    private boolean[] inputs;
    private float[] rotation;
}
