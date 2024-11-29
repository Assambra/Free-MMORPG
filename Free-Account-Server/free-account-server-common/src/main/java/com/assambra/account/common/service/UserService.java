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
    private final UserRepo userRepo;

    public User getUser(String username) {
        return userRepo.findByField("username", username);
    }
}
