package com.assambra.game.app.service;

import com.assambra.game.common.entity.User;
import com.assambra.game.common.repository.UserRepo;
import com.tvd12.ezyfox.bean.annotation.EzySingleton;
import lombok.AllArgsConstructor;
import lombok.Setter;

@Setter
@AllArgsConstructor
@EzySingleton("userService")
public class UserService {

    private final UserRepo userRepo;

    public User GetUserByUsername(String username)
    {
        return userRepo.findByField("username", username);
    }
}
