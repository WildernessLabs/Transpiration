using System;
using Meadow.Devices;
using Meadow.Gateways.Bluetooth;

namespace Transpiration.Services;

public class BLEService
{
    Definition bleDefinition;
    CharacteristicInt32 workoutIntervalCharacteristic;
    CharacteristicInt32 restIntervalCharacteristic;
    CharacteristicInt32 transpirationModeCharacteristic;
    CharacteristicBool isRunningCharacteristic;

    readonly string WorkoutIntervalUUID = "4e32d9e5-a459-407b-a2ea-91773328d0d6";
    readonly string RestIntervalUUID = "cd1077db-ea61-4a23-be06-3b67a5602ab9";
    readonly string TranspirationModeUUID = "29ce2fa4-3ef3-47d1-bea0-046545bd8e71";
    readonly string IsRunningUUID = "b1141f55-be82-4431-ba8a-79fcded9acd4";

    public event CharacteristicValueSetHandler WorkoutIntervalChanged;
    public event CharacteristicValueSetHandler RestIntervalChanged;
    public event CharacteristicValueSetHandler IsRunningChanged;
    public event CharacteristicValueSetHandler ModeChanged;

    public void SetWorkoutInterval(int interval)
    {
        workoutIntervalCharacteristic?.SetValue(interval);
    }

    public void SetRestInterval(int interval)
    {
        restIntervalCharacteristic?.SetValue(interval);
    }

    public void Initialize(F7Micro device)
    {
        // initialize the bluetooth defnition tree
        Console.WriteLine("Starting the BLE server");
        bleDefinition = GetDefinition();
        device.BluetoothAdapter.StartBluetoothServer(bleDefinition);
    }

    protected Definition GetDefinition()
    {
        isRunningCharacteristic = new CharacteristicBool(
                name: "isRunning",
                uuid: IsRunningUUID,
                permissions: CharacteristicPermission.Read | CharacteristicPermission.Write,
                properties: CharacteristicProperty.Read | CharacteristicProperty.Write);

        workoutIntervalCharacteristic = new CharacteristicInt32(
                name: "workoutInterval",
                uuid: WorkoutIntervalUUID,
                permissions: CharacteristicPermission.Read | CharacteristicPermission.Write,
                properties: CharacteristicProperty.Read | CharacteristicProperty.Write);

        restIntervalCharacteristic = new CharacteristicInt32(
                name: "restInterval",
                uuid: RestIntervalUUID,
                permissions: CharacteristicPermission.Read | CharacteristicPermission.Write,
                properties: CharacteristicProperty.Read | CharacteristicProperty.Write);

        transpirationModeCharacteristic = new CharacteristicInt32(
                name: "transpirationMode",
                uuid: TranspirationModeUUID,
                permissions: CharacteristicPermission.Read | CharacteristicPermission.Write,
                properties: CharacteristicProperty.Read | CharacteristicProperty.Write);

        workoutIntervalCharacteristic.ValueSet += (c, d) => WorkoutIntervalChanged?.Invoke(c, d);
        restIntervalCharacteristic.ValueSet += (c, d) => RestIntervalChanged?.Invoke(c, d);
        isRunningCharacteristic.ValueSet += (c, d) => IsRunningChanged?.Invoke(c, d);
        transpirationModeCharacteristic.ValueSet += (c, d) => ModeChanged?.Invoke(c, d);

        ICharacteristic[] characteristics = {
                workoutIntervalCharacteristic,
                restIntervalCharacteristic,
                isRunningCharacteristic,
                transpirationModeCharacteristic
        };

        var service = new Service(
                "Transpiration",
                500,
                characteristics);

        return new Definition("Transpiration", service);
    }
}