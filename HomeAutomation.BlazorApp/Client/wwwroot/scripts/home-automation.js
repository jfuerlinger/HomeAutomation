function changeDeviceByState(id, state) {
    console.log("ChangeDeviceByState was called");

    let device = document.getElementById(id);
    if (device) {
        switch (state) {
            case "ON":
                console.log("-> ON");
                device.classList.add("light-enabled");
                device.classList.remove("light-disabled");
                break;
            case "OFF":
                console.log("-> OFF");
                device.classList.add("light-disabled");
                device.classList.remove("light-enabled");
                break;
            default:
                console.log(`unknown state ${state}!`);
                break;
        }
    }
}