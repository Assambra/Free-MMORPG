package com.assambra.masterserver.common.service;

import com.assambra.masterserver.common.entity.Account;
import com.assambra.masterserver.common.repository.AccountRepo;
import com.tvd12.ezyfox.bean.annotation.EzySingleton;
import lombok.AllArgsConstructor;
import lombok.Setter;

@Setter
@AllArgsConstructor
@EzySingleton("userService")
public class UserService
{
    private final AccountRepo accountRepo;

    public Account getUser(String username) {
        return accountRepo.findByField("username", username);
    }
}
