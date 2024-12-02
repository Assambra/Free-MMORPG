package com.assambra.masterserver.app.request;

import com.tvd12.ezyfox.binding.annotation.EzyObjectBinding;
import lombok.Data;

@Data
@EzyObjectBinding
public class CreateCharacterRequest {
    private String name;
    private String sex;
    private String race;
    private String model;
}
