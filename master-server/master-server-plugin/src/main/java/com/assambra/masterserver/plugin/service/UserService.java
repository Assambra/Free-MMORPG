package com.assambra.masterserver.plugin.service;

import com.assambra.masterserver.common.entity.Account;
import com.assambra.masterserver.common.repository.AccountRepo;
import com.tvd12.ezyfox.bean.annotation.EzySingleton;
import lombok.AllArgsConstructor;

import java.util.List;


@AllArgsConstructor
@EzySingleton("userService")
public class UserService {

    private final AccountRepo accountRepo;
    private final MaxIdService maxIdService;

    public Account getUser(String username) {
        return accountRepo.findByField("username", username);
    }

    public List<Account> getAllUsers() {
        return accountRepo.findAll();
    }
}
