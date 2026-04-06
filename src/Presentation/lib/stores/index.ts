import { tourStore } from './tourStore';
import {UiStore} from './uiStore';
// Import other stores as you create them

export class RootStore {
  tourStore = tourStore;
  uiStore = UiStore;
  // Add other stores here: userStore, bookingStore, etc.
}

export const rootStore = new RootStore();