import { createContext } from "react"
//import CounterStore from "./counterStore"
import { UiStore } from "./uiStore";

interface Store {
    //counterStore: CounterStore,
    uiStore: UiStore
}

export const store:Store = {
   // counterStore : new CounterStore(),
    uiStore : new UiStore()
}

export const StoreContext = createContext(store); // using react context to make the Countstore globallt available. StoreContent will be available across the react app.