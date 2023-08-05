package com.assambra.common.service;

import com.assambra.common.entity.Account;
import com.assambra.common.repository.AccountRepo;
import com.tvd12.ezyfox.bean.annotation.EzySingleton;
import lombok.AllArgsConstructor;
import lombok.Setter;

@Setter
@AllArgsConstructor
@EzySingleton("accountService")
public class AccountService
{
    private final AccountRepo accountRepo;

    public Account getAccount(String username) {
        return accountRepo.findByField("username", username);
    }
}
