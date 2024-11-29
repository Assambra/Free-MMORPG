package com.assambra.account.common.service;

import com.assambra.account.common.entity.User;
import com.assambra.account.common.repository.UserRepo;
import com.tvd12.ezyfox.bean.annotation.EzySingleton;
import lombok.AllArgsConstructor;
import lombok.Setter;

@Setter
@AllArgsConstructor
@EzySingleton("userService")
public class UserService
{
    private final UserRepo accountRepo;

    public User getAccount(String username) {
        return accountRepo.findByField("username", username);
    }
}
