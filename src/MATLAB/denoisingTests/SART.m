

nProjections = length(Projections);

angleStep=360.0/nProjections;

sizeProjections = length(Projections{2});

jumpSize =floor( nProjections/4);

reconImageSIRT=reconImage;
clear filter;
B=0:(sizeProjections/2);
filter(1:length(B)) =B;
B=(sizeProjections/2):-1:1;
filter(length(filter):length(filter)+length(B)-1) =B;

filter=filter./(length(filter)/2)^2*3.14;

R_Thresh = (sizeProjections/2.1)^2;
centerI= sizeProjections/2;
centerJ= sizeProjections/2;
mask=zeros([sizeProjections sizeProjections]);

for I=1:sizeProjections
        for J=1:sizeProjections
            R= ((I-centerI)^2 + (J-centerJ)^2)/R_Thresh;
            if R>1
                mask(I,J)=1;
            end
        end
end
maskIndex = find(mask==1);

addition = zeros([sizeProjections sizeProjections]);
angleI=1;
figure;
colormap gray;
for K=1:5
    
    reconImageSIRT(maskIndex)=0;
    angles = randperm(nProjections);
    for I=1:nProjections
        angleI=angles(I);
        
        projection = Projections{angleI};

        tempImage = imrotate(reconImageSIRT,-1*angleStep*angleI,'bilinear','crop'); 
        
        forward = sum(tempImage);
        
        diff=(projection -forward)/sizeProjections;
   
        for J=1:sizeProjections
           addition(:,J)= diff(J);
        end

        addition=imrotate(addition,angleStep*angleI,'nearest','crop');

        reconImageSIRT=reconImageSIRT + addition;
    end
    imagesc(reconImageSIRT);drawnow;
  
end

imagesc(reconImageSIRT);colormap gray;


double(max(D(:)))/max(reconImageSIRT(:))