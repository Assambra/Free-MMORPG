package com.assambra.account.app.request;

import com.tvd12.ezyfox.binding.annotation.EzyObjectBinding;
import lombok.Data;

@Data
@EzyObjectBinding
public class ActivateAccountRequest
{
    private String username;
    private String activationCode;
}
