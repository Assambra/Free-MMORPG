package com.assambra.game.app.model;

import com.tvd12.ezyfox.binding.annotation.EzyArrayBinding;
import lombok.Builder;
import lombok.Getter;

@Getter
@Builder
@EzyArrayBinding
public class CharacterListModel {
    Long id;
    Long accountId;
    String name;
    String sex;
    String race;
    String model;
}
