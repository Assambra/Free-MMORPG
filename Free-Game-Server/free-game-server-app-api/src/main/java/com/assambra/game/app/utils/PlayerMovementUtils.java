package com.assambra.game.app.utils;

import com.assambra.game.app.model.PlayerInputModel;
import com.tvd12.gamebox.math.Vec3;
import org.apache.commons.math3.util.Precision;

public final class PlayerMovementUtils {

    public static Vec3 getNextPosition(Vec3 currentPosition, Vec3 rotation, PlayerInputModel model)
    {
        Vec3 movement = getMoveDirection(model.getInputs());
        movement.multiply(0.02 * 5.424);

        Vec3 forward = GetForwardDirection(rotation);

        forward.multiply(movement.z);
        Vec3 nextPosition = new Vec3(currentPosition);

        nextPosition.add(forward);

        return nextPosition;
    }

    public static Vec3 getNextRotation(Vec3 currentRotation, PlayerInputModel model)
    {
        Vec3 movement = getMoveDirection(model.getInputs());

        Vec3 nextRotation = new Vec3(0, movement.x, 0);
        nextRotation.multiply(0.02 * 100f);
        nextRotation.add(currentRotation);

        return nextRotation;
    }

    private static Vec3 getMoveDirection(boolean[] inputs)
    {
        Vec3 moveDirection = new Vec3();

        if(inputs[0])
            moveDirection.z += 1;
        if(inputs[1])
            moveDirection.z -= 1;
        if(inputs[2])
            moveDirection.x -= 1;
        if(inputs[3])
            moveDirection.x += 1;

        return moveDirection;
    }

    public static Vec3 GetForwardDirection(Vec3 rotation)
    {
        double xPos = Precision.round(Math.sin(Math.toRadians(rotation.y)) * Math.cos(Math.toRadians(rotation.x)), 2);
        double yPos = Precision.round(Math.sin(Math.toRadians(-rotation.x)), 2);
        double zPos = Precision.round(Math.cos(Math.toRadians(rotation.x)) * Math.cos(Math.toRadians(rotation.y)), 2);

        return new Vec3((float)xPos, (float)yPos, (float)zPos);
    }
}
