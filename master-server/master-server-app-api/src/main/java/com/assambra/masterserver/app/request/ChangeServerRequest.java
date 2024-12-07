package com.assambra.masterserver.app.request;

import com.tvd12.ezyfox.binding.annotation.EzyObjectBinding;
import com.tvd12.ezyfox.entity.EzyArray;
import lombok.Data;

@Data
@EzyObjectBinding
public class ChangeServerRequest {
    private Long playerId;
    private String room;
    private EzyArray position;
    private EzyArray rotation;
}
