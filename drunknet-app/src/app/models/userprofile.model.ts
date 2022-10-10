import { Gender } from "./gender.enum";

export class UserProfile {
    public userId: number = 0;
    public externalAuthId: string = "";
    public weight: number = 0;
    public gender: Gender = Gender.unknown;
    public name: string = '';
}