package com.assambra.masterserver.app.model.request;

import com.tvd12.ezyfox.entity.EzyArray;
import lombok.Builder;
import lombok.Getter;

@Getter
@Builder
public class RequestChangeServerModel {
    private Long playerId;
    private String room;
    private EzyArray position;
    private EzyArray rotation;
}
