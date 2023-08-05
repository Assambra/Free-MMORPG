package com.assambra.app.request;

import com.tvd12.ezyfox.binding.annotation.EzyObjectBinding;
import lombok.Data;

@Data
@EzyObjectBinding
public class ForgotPasswordRequest {
    private String usernameOrEMail;
}
