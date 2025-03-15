import { expect, vi } from "vitest";
import "@testing-library/jest-dom/vitest";

// Explicitly attach `expect` and `vi` globally
globalThis.expect = expect;
globalThis.vi = vi;
