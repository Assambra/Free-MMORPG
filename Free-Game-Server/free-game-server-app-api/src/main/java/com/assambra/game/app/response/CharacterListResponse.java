package com.assambra.game.app.response;

import com.tvd12.ezyfox.binding.annotation.EzyObjectBinding;
import com.tvd12.ezyfox.entity.EzyArray;
import lombok.Builder;
import lombok.Getter;

@Getter
@Builder
@EzyObjectBinding
public class CharacterListResponse {
    private EzyArray characters;
}
