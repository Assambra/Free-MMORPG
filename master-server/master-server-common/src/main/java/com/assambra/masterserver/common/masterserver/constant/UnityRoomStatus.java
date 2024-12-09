package com.assambra.masterserver.common.masterserver.constant;

import com.tvd12.ezyfox.util.EzyEnums;
import com.tvd12.gamebox.constant.IRoomStatus;
import lombok.Getter;

import java.util.Map;

public enum UnityRoomStatus implements IRoomStatus {

    STARTING(0),
    READY(1);

    @Getter
    private final int id;

    private static final Map<Integer, UnityRoomStatus> STATUS_BY_ID =
            EzyEnums.enumMapInt(UnityRoomStatus.class);

    UnityRoomStatus(int id) {
        this.id = id;
    }

    public static UnityRoomStatus valueOf(int id) {
        return STATUS_BY_ID.get(id);
    }

    @Override
    public String getName() {
        return toString();
    }
}
