package com.assambra.masterserver.app.service;

import com.assambra.masterserver.app.model.request.RequestCreateAccountModel;
import com.assambra.masterserver.common.entity.Account;
import com.assambra.masterserver.common.repository.AccountRepo;
import com.tvd12.ezyfox.bean.annotation.EzySingleton;
import com.tvd12.ezyfox.security.EzySHA256;
import lombok.AllArgsConstructor;
import lombok.Setter;

import java.util.Date;

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

    public Account getAccountByUsernameOrEMail(String usernameOrEMail)
    {
        Account account = getAccountByUsername(usernameOrEMail);
        if(account == null)
            account = getAccountByEMail(usernameOrEMail);

        return account;
    }

    public Account getAccountByUsername(String username) {
        return accountRepo.findByField("username", username);
    }

    public Account getAccountByEMail(String email) {
        return accountRepo.findByField("email", email);
    }

    public void createAccount(RequestCreateAccountModel model, String activationCode) {
        Account account = new Account();
        account.setId(maxIdService.incrementAndGet("account"));
        account.setEmail(model.getEmail());
        account.setUsername(model.getUsername());
        account.setPassword(encodePassword(model.getPassword()));
        account.setActivated(false);
        account.setActivationCode(activationCode);

        Date date = new Date();
        account.setCreationDate(date);

        account.setRole("Player");
        account.setMaxAllowedCharacters(3);

        accountRepo.save(account);
    }

    public Boolean activateAccount(String username, String activationCode)
    {
        Account account = getAccountByUsername(username);
        if(account.getActivationCode().contains(activationCode))
        {
            account.setActivated(true);
            accountRepo.save(account);

            return true;
        }
        else
            return false;
    }

    public void updatePassword(Long accountId, String newPassword) {
        accountRepo.updateStringFieldById(accountId, "password", encodePassword(newPassword));
    }

    private String encodePassword(String password)
    {
        return EzySHA256.cryptUtfToLowercase(password);
    }
}
