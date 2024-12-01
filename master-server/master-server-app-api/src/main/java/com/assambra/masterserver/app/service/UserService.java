package com.assambra.masterserver.app.service;

import com.assambra.masterserver.common.entity.User;
import com.assambra.masterserver.common.repository.UserRepo;
import com.tvd12.ezyfox.bean.annotation.EzySingleton;
import lombok.AllArgsConstructor;
import lombok.Setter;

import java.util.Date;

@Setter
@AllArgsConstructor
@EzySingleton("userService")
public class UserService
{
    private final UserRepo userRepo;
    private final MaxIdService maxIdService;

    public void updateStringFieldById(Long id, String field, String value) {
        userRepo.updateStringFieldById(id, field, value);
    }

    public User getFieldValueByFieldAndValue(String field, String value, String retValue) {
        return userRepo.getFieldValueByFieldAndValue(field, value, retValue);
    }

    public User getUserByUsername(String username) {
        return userRepo.findByField("username", username);
    }

    public User getUserByEMail(String email) {
        return userRepo.findByField("email", email);
    }

    public void createUser(String email, String username, String password, String activationCode) {
        User user = new User();
        user.setId(maxIdService.incrementAndGet("user"));
        user.setEmail(email);
        user.setUsername(username);
        user.setPassword(password);
        user.setActivated(false);
        user.setActivationCode(activationCode);

        Date date = new Date();
        user.setCreationDate(date);

        user.setRole("Player");
        user.setMaxAllowedCharacters(3);

        userRepo.save(user);
    }

    public Boolean activateUser(String username, String activationCode)
    {
        User user = getUserByUsername(username);
        if(user.getActivationCode().contains(activationCode))
        {
            user.setActivated(true);
            userRepo.save(user);

            return true;
        }
        else
            return false;
    }
}
