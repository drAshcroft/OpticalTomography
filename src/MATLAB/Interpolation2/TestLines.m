function [quality]=TestLines(slices,cutPlane, canvas, minX, minY)
L=size(slices{1}.array,2);

for I=1:length(slices)
    if isfield(slices{I},'Point1')==true
        A1=slices{I}.array(:,cutPlane);
        
        [x y]=MissPoint(slices{I}, 1:L);
        s=0;
        cc=0;
        for J=1:L
            i=round(x(J)-minX);
            j=round(y(J)-minY);
            if (isnan(i)==false && isnan(j)==false && isinf(i)==false && isinf(j)==false)
                
                if (i>1 && j>1 && i<L &&j<L)
                    c=canvas(i,j);
                    if c~=0
                        s=s+(c - A1(J))^2;
                        cc=cc+1;
                    else
                        % s=s+((c - A1(J))^2)/2;
                    end
                    
                end
            end
        end
        quality(I)=(s/(cc-1))^.5;
    else
        quality(I)=10000;
    end
end

end



