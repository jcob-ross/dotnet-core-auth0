export class User {
  private _name: string;
  private _email: string;
  private _pictureUrl: string;

  constructor(name: string, email: string, pictureUrl: string) {
    this._name = name;
    this._email = email;
    this._pictureUrl = pictureUrl;
  }

  get name(): string {
    return this._name;
  }

  get email(): string {
    return this._email;
  }

  get pictureUrl(): string {
    return this._pictureUrl;
  }

  get hasPicture(): boolean {
    return this._pictureUrl != null;
  }
}

export interface IClaim {
  type: string;
  value: string;
}

export interface IUserData {
  claims: IClaim[];
}

export interface IAuth0Identity {
  connection: string;
  isSocial: boolean;
  provider: string;
  user_id: string;
}

export interface IAuth0Profile {
  clientID: string;
  created_at: Date;
  global_client_id: string;

  user_id: string;
  name: string;
  email: string;
  emailVerified: boolean;
  picture: string;
  updated_at: Date;
  identities: IAuth0Identity[];
}
