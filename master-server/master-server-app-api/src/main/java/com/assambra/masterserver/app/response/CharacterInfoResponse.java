package com.assambra.masterserver.app.response;

import com.tvd12.ezyfox.binding.annotation.EzyObjectBinding;
import lombok.Builder;
import lombok.Getter;

@Getter
@Builder
@EzyObjectBinding
public class CharacterInfoResponse {
    private Long id;
    private String name;
    private String sex;
    private String race;
    private String room;
    private String model;
}
