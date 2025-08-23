import { createContext } from "react";
import { UIStore } from "./uistore";

interface Store {
    uiStore: UIStore
}

export const store:Store = {
    uiStore : new UIStore()
}

export const StoreContext = createContext(store); // using react context to make the Countstore globallt available. StoreContent will be available across the react app.