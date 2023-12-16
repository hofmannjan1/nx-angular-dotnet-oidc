/*
 * ABOUT THIS FILE
 *
 * This files provides an utility function to turn any array into a record by providing a key
 * selector function.
 */
type KeySelectorFn<T> = (t: T) => string | number | symbol;
type KeySelector<T> = KeySelectorFn<T> | keyof T;

export function createRecord<TKey extends string | number | symbol, TValue>(
  array: TValue[] | undefined,
  keySelector: KeySelector<TValue>
): Record<TKey, TValue> {
  const createKey = (item: TValue) =>
    typeof keySelector === "function" ? (keySelector(item) as TKey) : (item[keySelector] as TKey);

  let record = {} as Record<TKey, TValue>;
  array?.forEach((item) => {
    record[createKey(item)] = item;
  });

  return record;
}
