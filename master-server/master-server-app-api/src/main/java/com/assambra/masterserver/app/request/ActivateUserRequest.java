package com.assambra.masterserver.app.request;

import com.tvd12.ezyfox.binding.annotation.EzyObjectBinding;
import lombok.Data;

@Data
@EzyObjectBinding
public class ActivateUserRequest
{
    private String username;
    private String activationCode;
}
