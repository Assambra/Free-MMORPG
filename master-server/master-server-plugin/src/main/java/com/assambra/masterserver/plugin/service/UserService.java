package com.assambra.masterserver.plugin.service;

import com.assambra.masterserver.common.entity.User;
import com.assambra.masterserver.common.repository.UserRepo;
import com.tvd12.ezyfox.bean.annotation.EzySingleton;
import lombok.AllArgsConstructor;

import java.util.List;


@AllArgsConstructor
@EzySingleton("userService")
public class UserService {

    private final UserRepo userRepo;
    private final MaxIdService maxIdService;

    public User getUser(String username) {
        return userRepo.findByField("username", username);
    }

    public List<User> getAllUsers() {
        return userRepo.findAll();
    }
}
