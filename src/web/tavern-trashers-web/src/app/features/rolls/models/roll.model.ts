export interface RollValue {
  value: number;
  size: string;
}

export interface Roll {
  id: string;
  expression: string;
  total: number;
  minimum: number;
  maximum: number;
  average: number;
  rawRolls: RollValue[];
  keptRolls: RollValue[];
  rolledAtUtc: string;
  children: Roll[];
}
