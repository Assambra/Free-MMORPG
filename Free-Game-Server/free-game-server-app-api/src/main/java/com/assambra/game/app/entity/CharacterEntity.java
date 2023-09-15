package com.assambra.game.app.entity;

import com.tvd12.gamebox.math.Vec3;

public class CharacterEntity {
    public String accountUsername;
    public Long roomId;
    public String characterName;
    public String characterModel;
    public Vec3 position;
    public Vec3 rotation;

    public CharacterEntity(String accountUsername, Long roomId, String characterName, String characterModel, Vec3 position,Vec3 rotation)
    {
        this.accountUsername = accountUsername;
        this.roomId = roomId;
        this.characterName = characterName;
        this.characterModel = characterModel;
        this.position = position;
        this.rotation = rotation;
    }
}
