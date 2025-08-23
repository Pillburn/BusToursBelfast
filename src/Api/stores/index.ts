import { tourStore } from './tourStore';
import {uiStore} from './uiStore';
// Import other stores as you create them

export class RootStore {
  tourStore = tourStore;
  uiStore = uiStore;
  // Add other stores here: userStore, bookingStore, etc.
}

export const rootStore = new RootStore();