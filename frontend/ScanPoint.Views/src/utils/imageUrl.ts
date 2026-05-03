const API_BASE = "http://localhost:5055";
 
export function getProfileImageUrl(profileImagePath?: string | null): string {
  if (!profileImagePath) return "/images/user/owner.jpg";
 
  // Nëse është URL e plotë, kthejen direkt
  if (profileImagePath.startsWith("http")) return profileImagePath;
 
  // Hiq / nga fillimi nëse ekziston, pastaj ndërtoje URL
  const cleanPath = profileImagePath.replace(/^\/+/, "");
  return `${API_BASE}/${cleanPath}`;
}