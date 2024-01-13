package com.assambra.account.app.request;

import com.tvd12.ezyfox.binding.annotation.EzyObjectBinding;
import lombok.Data;

@Data
@EzyObjectBinding
public class ResendActivationMailRequest {
    private String username;
}
