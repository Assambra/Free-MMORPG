package com.assambra.account.common.service;

import com.assambra.account.common.entity.Account;
import com.assambra.account.common.repository.AccountRepo;
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
