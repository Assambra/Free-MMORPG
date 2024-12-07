package com.assambra.masterserver.app.request;

import com.tvd12.ezyfox.binding.annotation.EzyObjectBinding;
import lombok.Data;

@Data
@EzyObjectBinding
public class PlayRequest {
    private Long playerId;
}
