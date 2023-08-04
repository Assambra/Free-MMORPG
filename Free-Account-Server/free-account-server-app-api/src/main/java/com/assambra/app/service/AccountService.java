package com.assambra.app.service;

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
    private final MaxIdService maxIdService;


    public void updateStringFieldById(Long id, String field, String value) {
        accountRepo.updateStringFieldById(id, field, value);
    }


    public Account getFieldValueByFieldAndValue(String field, String value, String retValue) {
        return accountRepo.getFieldValueByFieldAndValue(field, value, retValue);
    }

    public Account getAccountByUsername(String username) {
        return accountRepo.findByField("username", username);
    }

    public Account getAccountByEMail(String email) {
        return accountRepo.findByField("email", email);
    }

    public void createAccount(String email, String username, String password) {
        Account account = new Account();
        account.setId(maxIdService.incrementAndGet("account"));
        account.setEmail(email);
        account.setUsername(username);
        account.setPassword(password);
        accountRepo.save(account);
    }
}
