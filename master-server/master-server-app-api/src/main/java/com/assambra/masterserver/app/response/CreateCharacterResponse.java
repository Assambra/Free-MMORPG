package com.assambra.masterserver.app.response;

import com.tvd12.ezyfox.binding.annotation.EzyObjectBinding;
import lombok.Builder;
import lombok.Getter;

@Getter
@Builder
@EzyObjectBinding
public class CreateCharacterResponse {
    private Long id;
    private String result;
}
