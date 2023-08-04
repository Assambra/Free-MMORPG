package com.assambra.plugin.service;

import com.assambra.common.entity.Account;
import com.assambra.common.repository.AccountRepository;
import com.tvd12.ezyfox.bean.annotation.EzySingleton;
import lombok.AllArgsConstructor;
import lombok.Setter;

@Setter
@AllArgsConstructor
@EzySingleton("accountService")
public class AccountService {
    private final AccountRepository accountRepository;

    public Account getAccount(String username) {
        return accountRepository.findByField("username", username);
    }
}
